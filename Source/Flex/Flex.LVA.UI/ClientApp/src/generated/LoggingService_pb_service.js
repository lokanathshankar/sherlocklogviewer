// package: Flex.LVA.Communication.v1
// file: LoggingService.proto

var LoggingService_pb = require("./LoggingService_pb");
var CommonReturnTypes_pb = require("./CommonReturnTypes_pb");
var grpc = require("@improbable-eng/grpc-web").grpc;

var LoggingService = (function () {
  function LoggingService() {}
  LoggingService.serviceName = "Flex.LVA.Communication.v1.LoggingService";
  return LoggingService;
}());

LoggingService.Debug = {
  methodName: "Debug",
  service: LoggingService,
  requestStream: false,
  responseStream: false,
  requestType: LoggingService_pb.LoggingRequest,
  responseType: CommonReturnTypes_pb.VoidMessage
};

LoggingService.Info = {
  methodName: "Info",
  service: LoggingService,
  requestStream: false,
  responseStream: false,
  requestType: LoggingService_pb.LoggingRequest,
  responseType: CommonReturnTypes_pb.VoidMessage
};

LoggingService.Error = {
  methodName: "Error",
  service: LoggingService,
  requestStream: false,
  responseStream: false,
  requestType: LoggingService_pb.LoggingRequest,
  responseType: CommonReturnTypes_pb.VoidMessage
};

LoggingService.Verbose = {
  methodName: "Verbose",
  service: LoggingService,
  requestStream: false,
  responseStream: false,
  requestType: LoggingService_pb.LoggingRequest,
  responseType: CommonReturnTypes_pb.VoidMessage
};

LoggingService.Warn = {
  methodName: "Warn",
  service: LoggingService,
  requestStream: false,
  responseStream: false,
  requestType: LoggingService_pb.LoggingRequest,
  responseType: CommonReturnTypes_pb.VoidMessage
};

exports.LoggingService = LoggingService;

function LoggingServiceClient(serviceHost, options) {
  this.serviceHost = serviceHost;
  this.options = options || {};
}

LoggingServiceClient.prototype.debug = function debug(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(LoggingService.Debug, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

LoggingServiceClient.prototype.info = function info(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(LoggingService.Info, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

LoggingServiceClient.prototype.error = function error(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(LoggingService.Error, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

LoggingServiceClient.prototype.verbose = function verbose(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(LoggingService.Verbose, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

LoggingServiceClient.prototype.warn = function warn(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(LoggingService.Warn, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

exports.LoggingServiceClient = LoggingServiceClient;

