OccupOS.SensorData = DS.Model.extend({
    measuredData:             DS.attr('string'),
    createddAt:               DS.attr('date'),
    updatedAt:                DS.attr('date'),
    sensorMetadataId:         DS.belongsTo('OccupOS.SensorMetadata'),
    IntermediateHwMedadataId: DS.belongsTo('OccupOS.IntermediateHwMedadata')
});