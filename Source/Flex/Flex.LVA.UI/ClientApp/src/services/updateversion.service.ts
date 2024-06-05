
import { Injectable } from "@angular/core";
import { grpc } from "@improbable-eng/grpc-web";
import { Observable, Observer, Subject } from "rxjs";
import { GlobalProgressBarComponent } from "../app/global-progress-bar/global-progress-bar.component";
import { GlobalProgressBarEvents } from "../app/global-progress-bar/GloblaProgressBarEvents";
import { ILogHeader } from "../common/loggraph";
import { ISymanticLogs } from "../common/loggraph";
import { BoolMessage, IntMessage, RegistrationData, StringMessage, VoidMessage } from "../generated/CommonReturnTypes_pb";
import { LoggingRequest } from "../generated/LoggingService_pb";
import { LoggingServiceClient, ServiceError } from "../generated/LoggingService_pb_service";
import { RegistrationService, RegistrationServiceClient } from "../generated/RegistrationService_pb_service";
import { Tracer } from "../utils/tracer";
import { ServiceSettings } from "./constants";
import { Engine } from "./engine.service";
import { Logger } from "./logger.service";
import { MenuEvents } from '../app/top-menu/TopMenuEvents';

@Injectable({
  providedIn: 'root'
})
export class UpdateVersionChecker {
  private myDomain: string = "UpdateVersionChecker";
  private myRegClient: RegistrationServiceClient;
  constructor(private myLogger: Logger, private myGlobalProgressBar: GlobalProgressBarEvents, private menuEvents: MenuEvents) {
    this.myRegClient = new RegistrationServiceClient(ServiceSettings.GRPC_REGISTERAR_ENDPOINT);
  }

  IsUpdateAvailable() {
    const aTrace = new Tracer(this.myLogger, this.myDomain, "IsUpdateAvailable");
    this.myRegClient.readServiceVersion(new VoidMessage(), (error: ServiceError | null, responseMessage: StringMessage | null) => {
      if (error != null) {
        aTrace.error(`Error startServices Client : ${error}`);
        return false;
      }

      if (responseMessage == null) {
        aTrace.error('Null Response Message Is UnAcceptable');
        return false;
      }

      if (responseMessage.getValue()?.length == 0) {
        aTrace.error('Engine ID Cannot Be Zero');
        return false;
      }
      const versionNumber = responseMessage.getValue();
      this.menuEvents.isUpdateAvailable.next(versionNumber);
      console.log(responseMessage);
      // Fire a event with update details
      return true;
    });
  }

  OpenLocalBrowser() {
    const aTrace = new Tracer(this.myLogger, this.myDomain, "OpenLocalBrowser");
    const aUrl = new StringMessage();
    aUrl.setValue("https://sherlocklogviewer.in");
    this.myRegClient.openWebPage(aUrl, (error: ServiceError | null, responseMessage: VoidMessage | null) => {
      if (error != null) {
        aTrace.error(`Error startServices Client : ${error}`);
        return false;
      }

      if (responseMessage == null) {
        aTrace.error('Null Response Message Is UnAcceptable');
        return false;
      }           
      return true;
    });
  }
}
