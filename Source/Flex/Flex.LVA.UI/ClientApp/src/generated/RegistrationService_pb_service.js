// package: Flex.LVA.Communication.v1
// file: RegistrationService.proto

var RegistrationService_pb = require("./RegistrationService_pb");
var CommonReturnTypes_pb = require("./CommonReturnTypes_pb");
var grpc = require("@improbable-eng/grpc-web").grpc;

var RegistrationService = (function () {
  function RegistrationService() {}
  RegistrationService.serviceName = "Flex.LVA.Communication.v1.RegistrationService";
  return RegistrationService;
}());

RegistrationService.StopServices = {
  methodName: "StopServices",
  service: RegistrationService,
  requestStream: false,
  responseStream: false,
  requestType: CommonReturnTypes_pb.RegistrationData,
  responseType: CommonReturnTypes_pb.BoolMessage
};

RegistrationService.StartServices = {
  methodName: "StartServices",
  service: RegistrationService,
  requestStream: false,
  responseStream: false,
  requestType: CommonReturnTypes_pb.VoidMessage,
  responseType: CommonReturnTypes_pb.RegistrationData
};

RegistrationService.ReadServiceVersion = {
  methodName: "ReadServiceVersion",
  service: RegistrationService,
  requestStream: false,
  responseStream: false,
  requestType: CommonReturnTypes_pb.VoidMessage,
  responseType: CommonReturnTypes_pb.StringMessage
};

RegistrationService.OpenWebPage = {
  methodName: "OpenWebPage",
  service: RegistrationService,
  requestStream: false,
  responseStream: false,
  requestType: CommonReturnTypes_pb.StringMessage,
  responseType: CommonReturnTypes_pb.VoidMessage
};

exports.RegistrationService = RegistrationService;

function RegistrationServiceClient(serviceHost, options) {
  this.serviceHost = serviceHost;
  this.options = options || {};
}

RegistrationServiceClient.prototype.stopServices = function stopServices(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(RegistrationService.StopServices, {
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

RegistrationServiceClient.prototype.startServices = function startServices(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(RegistrationService.StartServices, {
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

RegistrationServiceClient.prototype.readServiceVersion = function readServiceVersion(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(RegistrationService.ReadServiceVersion, {
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

RegistrationServiceClient.prototype.openWebPage = function openWebPage(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(RegistrationService.OpenWebPage, {
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

exports.RegistrationServiceClient = RegistrationServiceClient;

