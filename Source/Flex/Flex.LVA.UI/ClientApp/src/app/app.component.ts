import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { DateTime } from 'luxon';
import { ServiceSettings } from '../services/constants';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent {
  title = 'app';
  constructor(private myHttpClient: HttpClient) {
    // @ts-ignore
    if (!window.DateTime) {
      // @ts-ignore
      window.DateTime = DateTime;
    }
    // @ts-ignore
    if (!window.luxon) {
      // @ts-ignore
      window.luxon = DateTime;
    }

    var port;
    if (process?.env?.NODE_ENV === 'production') {
      port = window.location.port
    }
    else {
      port = "5110";
    }

    ServiceSettings.GRPC_ENGINE_ENDPOINT = ServiceSettings.GRPC_ENGINE_ENDPOINT + port;
    ServiceSettings.GRPC_LOGGER_ENDPOINT = ServiceSettings.GRPC_LOGGER_ENDPOINT + port;
    ServiceSettings.GRPC_REGISTERAR_ENDPOINT = ServiceSettings.GRPC_REGISTERAR_ENDPOINT + port;
    ServiceSettings.GRPC_RENDERER_ENDPOINT = ServiceSettings.GRPC_RENDERER_ENDPOINT + port;
  }
}
