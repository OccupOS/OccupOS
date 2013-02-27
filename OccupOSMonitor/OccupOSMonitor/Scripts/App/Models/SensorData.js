OccupOS.Sensordata = DS.Model.extend({
    measuredData: DS.attr('string'),
    measuredAt: DS.attr('date'),
    measuredData2: DS.attr('string')
    /*sendAt:                   DS.attr('date'),
    polledAt:                 DS.attr('date'),
    createddAt:               DS.attr('date'),
    updatedAt:                DS.attr('date'),
    sensorMetadataId:         DS.belongsTo('OccupOS.SensorMetadata'),
    IntermediateHwMedadataId: DS.belongsTo('OccupOS.IntermediateHwMedadata')*/
});