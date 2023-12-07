using AutoMapper;
using Common.Mongodb.Collections;
using Common.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Queries;
using WE.SharedService.Application.Constants;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Alarm;
using WE.Shared.Service.Domain.Entities;
using WE.Shared.Service.Domain.Repository;

namespace WE.SharedService.Application.Services
{
    public interface IAlarmService
    {
        public Task<AlarmDto> Create(CreateAlarmModel model);
        public Task<List<AlarmDto>> GetAll();
        public Task<PagedList<AlarmDto>> Search(SearchAlarmModel model);
        public Task<bool> Update(string id, UpdateAlarmModel moidel);
        public Task<bool> Delete(string id);
        public Task<List<AlarmNotificationDto>> GetAlarmByDeviceId(string id);

        public Task<List<AlarmNotificationDto>> GetAlarmByDeviceIdUsingDatabaseLayer(string? id, SearchAlarmModel search);

        public Task UpdateAlarmStatus(List<string> ids, string status);
        public Task<List<Alarm>> GetAlarmByIds(List<string> ids);
    }

    public class AlarmService : IAlarmService
    {
        private readonly IAlarmRepository _alarmRepository;
        private readonly IMapper _mapper;
        private readonly IDevicesRepository _deviceRepository;
        private readonly IDeviceStatusService _deviceStatusService;

        public AlarmService(
            IAlarmRepository alarmRepository, 
            IMapper mapper, 
            IDevicesRepository deviceRepository,
            IDeviceStatusService deviceStatusService
            )
        {
            _alarmRepository = alarmRepository;
            _mapper = mapper;
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
            this._deviceStatusService= deviceStatusService ?? throw new ArgumentNullException(nameof(deviceStatusService));
        }

        public async Task UpdateAlarmStatus(List<string> ids, string status)
        {
            var listItem = await _alarmRepository.FindAsync(x => ids.Contains(x.Id));

            if (listItem.Any())
            {
                foreach (var alarm in listItem)
                {
                    alarm.Status = status;
                    await _alarmRepository.UpdateAsync(alarm, x => x.Id == alarm.Id);
                }
            }
        }

        public async Task<AlarmDto> Create(CreateAlarmModel model)
        {
            var entity = _mapper.Map<Alarm>(model);

            entity.UpdatedDate = DateTime.UtcNow;

            var result = await _alarmRepository.InsertAsync(entity);

            if (result != null)
            {
                return _mapper.Map<AlarmDto>(result);
            }

            return null;
        }

        public async Task<bool> Delete(string id)
        {
            await _alarmRepository.DeleteAsync(x => x.Id == id);
            return true;
        }

        public async Task<List<AlarmDto>> GetAll()
        {
            var result = _alarmRepository.GetAllWithDataAuthorize().ToList();

            var response = _mapper.Map<List<AlarmDto>>(result);

            return response;
        }

        public async Task<PagedList<AlarmDto>> Search(SearchAlarmModel model)
        {
            var result = _alarmRepository.GetAll();

            var mappedList = _mapper.Map<PagedList<AlarmDto>>(await result.ToPagedList(model));

            return mappedList;
        }


        public async Task<bool> Update(string id, UpdateAlarmModel model)
        {
            var entity = _alarmRepository.GetFirstOrDefault(x => x.Id == id);

            if (entity != null)
            {
                var alarm = _mapper.Map<Alarm>(model);
                alarm.UpdatedDate = DateTime.UtcNow;
                alarm.Id = id;

                await _alarmRepository.UpdateAsync(alarm, x => x.Id == alarm.Id);

                return true;
            }
            return false;
        }

        public async Task<List<AlarmNotificationDto>> GetAlarmByDeviceId(string id)
        {
            var result = new List<AlarmNotificationDto>();
            var lstAlarm = new List<Alarm>();
            if (!string.IsNullOrEmpty(id))
            {
                var lstparam = new List<Tuple<string, object, FilterOperator>>()
                {
                    new Tuple<string, object, FilterOperator>("Source._id", id, FilterOperator.IsEqualTo)
                };
                var paramBuilder = BuildFilter<Alarm>(lstparam);

                lstAlarm = await _alarmRepository.FindBuilderAsync<Alarm>(paramBuilder);
            }
            else
            {
                lstAlarm = await _alarmRepository.FindBuilderAsync<Alarm>(null);
            }

            //summary Larm information
            result = lstAlarm.GroupBy(p => new { p.Source.Id, p.Text, p.Severity }).Select(g => new AlarmNotificationDto()
            {
                Severity = g.OrderByDescending(t => t.Time).First().Severity,
                CreationTime = g.OrderByDescending(t => t.Time).First().CreatedDate,
                Count = g.Count(),
                History = new History(),
                Source = new DeviceInfo()
                {
                    Id = g.OrderByDescending(t => t.Time).First().Source.Id,
                    Name = g.OrderByDescending(t => t.Time).First().Source.Name
                },
                Type = g.OrderByDescending(t => t.Time).First().Type,
                Time = g.OrderByDescending(t => t.Time).First().Time,
                Text = g.OrderByDescending(t => t.Time).First().Text,
                Status = g.OrderByDescending(t => t.Time).First().Status,
                Id = g.OrderByDescending(t => t.Time).First().Id,
            }).ToList();

            return result;
        }

        public async Task<List<AlarmNotificationDto>> GetAlarmByDeviceIdUsingDatabaseLayer(string? id, SearchAlarmModel search)
        {

            var listFilter = new List<BsonDocument>();


            if(id == null)
            {
                var currentDate = DateTime.UtcNow;
                var startDate = currentDate.AddDays(-7);
                var dateTimeMatch = new BsonDocument("$match",
                                new BsonDocument("Time",
                                new BsonDocument
                                        {
                                            { "$gt",
                                startDate },
                                            { "$lt",
                                currentDate }
                                        }));
                listFilter.Add(dateTimeMatch);
            }
            else
            {
                var currentDate = DateTime.UtcNow;
                var startDate = currentDate.AddDays(-30);
                var dateTimeMatch = new BsonDocument("$match",
                                new BsonDocument("Time",
                                new BsonDocument
                                        {
                                            { "$gt",
                                startDate },
                                            { "$lt",
                                currentDate }
                                        }));

                listFilter.Add(dateTimeMatch);
            }

            var group = new BsonDocument("$group",
    new BsonDocument
        {
            { "_id",
    new BsonDocument
            {
                { "DeviceId", "$Source._id" },
                { "Text", "$Text" },
                { "Severity", "$Severity" }
            } },
            { "Count",
    new BsonDocument("$sum", 1) },
            { "ObjectId",
    new BsonDocument("$last", "$_id") },
            { "Severity",
    new BsonDocument("$last", "$Severity") },
            { "CreationTime",
    new BsonDocument("$last", "$CreatedDate") },
            { "Type",
    new BsonDocument("$last", "$Type") },
            { "Time",
    new BsonDocument("$last", "$Time") },
            { "Text",
    new BsonDocument("$last", "$Text") },
            { "Status",
    new BsonDocument("$last", "$Status") },
            { "Source",
    new BsonDocument("$last",
    new BsonDocument
                {
                    { "Id", "$Source._id" },
                    { "Name", "$Source.Name" }
                }) }
        });

            var project = new BsonDocument("$project",
    new BsonDocument
        {
            { "_id", "$ObjectId" },
            { "Count", "$Count" },
            { "Severity", "$Severity" },
            { "CreationTime", "$CreationTime" },
            { "Type", "$Type" },
            { "Time", "$Time" },
            { "Text", "$Text" },
            { "Status", "$Status" },
            { "Source", "$Source" }
        });

            var sort = new BsonDocument("$sort",
    new BsonDocument
        {
            { "Time", -1 }
        });

            listFilter.Add(group);
            listFilter.Add(project);
            listFilter.Add(sort);

            PipelineDefinition<Alarm, AlarmNotificationDto> pipeline = listFilter.ToArray();

            try
            {

                var data = _alarmRepository.Collection.Aggregate<AlarmNotificationDto>(pipeline).ToList();

                var listActiveDevice = await _deviceStatusService.GetActiveDevice(new List<string>());

                var result = data.ToList().Where(
                    x => (x.Source.Id == null || listActiveDevice.Contains(x.Source.Id))
                    ).ToList();

                if (id != null)
                {

                    //var deviceName = (await _deviceRepository.FindAsync(x => x.SourceId == id)).ToList().FirstOrDefault().Name;
                    //result = result.Where(x => x.Source.Name == deviceName && (x.Status == AlarmConstant.Active || x.Status == AlarmConstant.Acknowledged)).ToList();

                    List<string> sourceids = id.Split(',').ToList();
                    var x = (await _deviceRepository.FindAsync(x => sourceids.Contains(x.SourceId))).ToList();
                    
                    List<string> deviceNames = new List<string>();
                    foreach (var a in x)
                    {
                        if (a != null) { deviceNames.Add(a.Name); }
                    }

                    result = result.Where(x => deviceNames.Contains(x.Source.Name) && (x.Status == AlarmConstant.Active || x.Status == AlarmConstant.Acknowledged)).ToList();
                }

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Alarm>> GetAlarmByIds(List<string> ids)
        {
            var listAlarm = (await _alarmRepository.FindAsync(x => ids.Contains(x.Id))).ToList();
            return listAlarm;
        }

        #region private

        private FilterDefinition<T> BuildFilter<T>(List<Tuple<string, object, FilterOperator>>? filterparam)
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Empty;

            foreach (var item in filterparam)
            {
                switch (item.Item3)
                {
                    case FilterOperator.IsNotEqualTo:
                        {
                            var filteritem = builder.Ne(item.Item1, item.Item2);
                            filter &= filteritem;
                        }
                        break;
                    case FilterOperator.IsLessThan:
                        {
                            var filteritem = builder.Lt(item.Item1, item.Item2);
                            filter &= filteritem;
                        }
                        break;
                    case FilterOperator.IsGreaterThan:
                        {
                            var filteritem = builder.Gt(item.Item1, item.Item2);
                            filter &= filteritem;
                        }
                        break;
                    case FilterOperator.Contains:
                        {
                            var filteritem = builder.Regex(item.Item1, new BsonRegularExpression(item.Item2.ToString()));
                            filter &= filteritem;
                        }
                        break;
                    default:
                        {
                            var filteritem = builder.Eq(item.Item1, item.Item2);
                            filter &= filteritem;
                        }
                        break;
                }

            }
            return filter;
        }

       
        #endregion
    }
}
