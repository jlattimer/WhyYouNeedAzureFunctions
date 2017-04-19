var moment = require('moment');

module.exports = function (context, req) {

    context.log("Received date", req.query.date);
    context.log("Received format", req.query.format);

    var formatted = moment(req.query.date).format(req.query.format);

    context.res = {
        status: 200,
        body: formatted
    };
    context.done();
};