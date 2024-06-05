// package: Flex.LVA.Communication.v1
// file: EngineService.proto

var EngineService_pb = require("./EngineService_pb");
var CommonReturnTypes_pb = require("./CommonReturnTypes_pb");
var grpc = require("@improbable-eng/grpc-web").grpc;

var EngineService = (function () {
  function EngineService() {}
  EngineService.serviceName = "Flex.LVA.Communication.v1.EngineService";
  return EngineService;
}());

EngineService.GetRawLogs = {
  methodName: "GetRawLogs",
  service: EngineService,
  requestStream: false,
  responseStream: false,
  requestType: EngineService_pb.GetRawLogsRequest,
  responseType: CommonReturnTypes_pb.StringMessage
};

EngineService.GetRawLog = {
  methodName: "GetRawLog",
  service: EngineService,
  requestStream: false,
  responseStream: false,
  requestType: EngineService_pb.GetRawLogRequest,
  responseType: CommonReturnTypes_pb.StringMessage
};

EngineService.PrepareLogs = {
  methodName: "PrepareLogs",
  service: EngineService,
  requestStream: false,
  responseStream: false,
  requestType: CommonReturnTypes_pb.RegistrationData,
  responseType: CommonReturnTypes_pb.VoidMessage
};

EngineService.AppendLogChunk = {
  methodName: "AppendLogChunk",
  service: EngineService,
  requestStream: false,
  responseStream: false,
  requestType: EngineService_pb.LogChunkRequest,
  responseType: CommonReturnTypes_pb.VoidMessage
};

EngineService.PrepareResources = {
  methodName: "PrepareResources",
  service: EngineService,
  requestStream: false,
  responseStream: false,
  requestType: EngineService_pb.PrepareResourceRequest,
  responseType: CommonReturnTypes_pb.VoidMessage
};

exports.EngineService = EngineService;

function EngineServiceClient(serviceHost, options) {
  this.serviceHost = serviceHost;
  this.options = options || {};
}

EngineServiceClient.prototype.getRawLogs = function getRawLogs(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(EngineService.GetRawLogs, {
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

EngineServiceClient.prototype.getRawLog = function getRawLog(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(EngineService.GetRawLog, {
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

EngineServiceClient.prototype.prepareLogs = function prepareLogs(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(EngineService.PrepareLogs, {
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

EngineServiceClient.prototype.appendLogChunk = function appendLogChunk(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(EngineService.AppendLogChunk, {
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

EngineServiceClient.prototype.prepareResources = function prepareResources(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(EngineService.PrepareResources, {
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

exports.EngineServiceClient = EngineServiceClient;

