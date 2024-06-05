import { Component, OnInit } from '@angular/core';
import { LoggingRequest } from '../../generated/LoggingService_pb';
import { LoggingService, LoggingServiceClient, ServiceError } from '../../generated/LoggingService_pb_service';
import { VoidMessage } from '../../generated/CommonReturnTypes_pb';
import { grpc } from "@improbable-eng/grpc-web";
import { TabulatorFull as Tabulator } from 'tabulator-tables';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  constructor() {
    this.ngOnInit()
    
  }

  ngOnInit(): void {
  }
}
