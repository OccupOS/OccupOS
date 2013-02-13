OccupOS.IntermediateHwMedadata = DS.Model.extend({
    externalId:     DS.attr('string'),
    departmentName: DS.attr('string'),
    buildingName:   DS.attr('string'),
    floorNr:        DS.attr('number'),
    creatorId:      DS.attr('number'),
    updaterId:      DS.attr('number'),
    createddAt:     DS.attr('date'),
    updatedAt:      DS.attr('date'),
    sensorMetadata: DS.hasMany('OccupOS.SensorMetadata'),
    sensorData:     DS.hasMany('OccupOs.SensorData')
});