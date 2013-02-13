OccupOS.SensorMetadata = DS.Model.extend({
    externalId:               DS.attr('string'),
    sensorName:               DS.attr('string'),
    roomId:                   DS.attr('string'),
    floorNr:                  DS.attr('number'),
    geoLongitude:             DS.attr('number'),
    geoLatidude:              DS.attr('number'),
    creatorId:                DS.attr('number'),
    updaterId:                DS.attr('number'),
    createddAt:               DS.attr('date'),
    updatedAt:                DS.attr('date'),
    intermediateHwMedadataId: DS.belongsTo('OccupOS.IntermediateHwMedadata')
});