"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.Logger = void 0;
var core_1 = require("@angular/core");
var LoggingService_pb_service_1 = require("../generated/LoggingService_pb_service");
var constants_1 = require("./constants");
var Logger = /** @class */ (function () {
    function Logger() {
        this.client = new LoggingService_pb_service_1.LoggingServiceClient(constants_1.ServiceSettings.GRPC_LOGGER_ENDPOINT);
    }
    Logger.prototype.info = function (logMessageRequest) {
        console.info(logMessageRequest.getDomain() + "." + logMessageRequest.getFunction() + " - " + logMessageRequest.getTracemessage());
        this.client.info(logMessageRequest, function (error, responseMessage) {
            if (error) {
                console.error(error);
            }
        });
    };
    Logger.prototype.error = function (logMessageRequest) {
        console.error(logMessageRequest.getDomain() + "." + logMessageRequest.getFunction() + " - " + logMessageRequest.getTracemessage());
        this.client.error(logMessageRequest, function (error, responseMessage) {
            if (error) {
                console.error(error);
            }
        });
    };
    Logger.prototype.debug = function (logMessageRequest) {
        this.client.debug(logMessageRequest, function (error, responseMessage) {
            if (error) {
                console.error(error);
            }
        });
    };
    Logger.prototype.verbose = function (logMessageRequest) {
        this.client.verbose(logMessageRequest, function (error, responseMessage) {
            if (error) {
                console.error(error);
            }
        });
    };
    Logger.prototype.warn = function (logMessageRequest) {
        this.client.warn(logMessageRequest, function (error, responseMessage) {
            if (error) {
                console.error(error);
            }
        });
    };
    Logger = __decorate([
        core_1.Injectable({
            providedIn: 'root'
        })
    ], Logger);
    return Logger;
}());
exports.Logger = Logger;
//# sourceMappingURL=logger.service.js.map