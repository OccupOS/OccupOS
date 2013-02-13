OccupOS.User = DS.Model.extend({
    username:   DS.attr('string'),
    email:      DS.attr('string'),
    password:   DS.attr('string'),
    firstName:  DS.attr('string'),
    lastName:   DS.attr('string'),
    creatorId:  DS.attr('number'),
    updaterId:  DS.attr('number'),
    createddAt: DS.attr('date'),
    updatedAt:  DS.attr('date')
});