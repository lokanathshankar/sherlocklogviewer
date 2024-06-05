import { Component } from '@angular/core';
import { Console } from 'console';
import { ParserEventsAndUpdates } from '../parser-setup-view/ParserEvents';
import { DefaultParserModel } from './DefaultParserModel';

@Component({
  selector: 'app-default-parser',
  templateUrl: './default-parser.component.html',
  styleUrls: ['./default-parser.component.css']
})

export class DefaultParserComponent {
  parserList: DefaultParserModel[] = [
    new DefaultParserModel('Android', '03-17 16:13:38.819  1702  8671 D PowerManagerService: acquire lock=233570404, flags=0x1, tag="View Lock", name=com.android.systemui, ws=null, uid=10037, pid=2227', '{ "Name": "AndroidLogs", "HeaderLineCount": 0, "Syntaxes": [ { "Id": 1, "BeginMarker": "", "EndMarker": "newline", "SyntaxType": 1, "Elements": [ { "Name": "Date", "EndSeparator": " ", "Type": 5, "ChildSyntaxId": null, "DateTimeFormat": "MM-dd" }, { "Name": "Time", "EndSeparator": " ", "Type": 4, "ChildSyntaxId": null, "DateTimeFormat": "HH:mm:ss.fff" }, { "Name": "ID1", "EndSeparator": " ", "Type": 1, "ChildSyntaxId": null, "DateTimeFormat": null }, { "Name": "ID2", "EndSeparator": " ", "Type": 1, "ChildSyntaxId": null, "DateTimeFormat": null }, { "Name": "Level", "EndSeparator": " ", "Type": 1, "ChildSyntaxId": null, "DateTimeFormat": null }, { "Name": "Method", "EndSeparator": ":", "Type": 1, "ChildSyntaxId": null, "DateTimeFormat": null }, { "Name": "Message", "EndSeparator": null, "Type": 1, "ChildSyntaxId": null, "DateTimeFormat": null } ] } ], "LogFileType": 1 }'),
    new DefaultParserModel('Apache', '[Sun Dec 04 04:47:44 2005] [notice] workerEnv.init() ok /etc/httpd/conf/workers2.properties', '{ "Name": "ApacheLog", "HeaderLineCount": 0, "Syntaxes": [ { "Id": 1, "BeginMarker": "[", "EndMarker": "newline", "SyntaxType": 1, "Elements": [ { "Name": "DateTime", "EndSeparator": "] [", "Type": 1, "ChildSyntaxId": null, "DateTimeFormat": null }, { "Name": "Level", "EndSeparator": "] ", "Type": 1, "ChildSyntaxId": null, "DateTimeFormat": null }, { "Name": "Method", "EndSeparator": " ", "Type": 1, "ChildSyntaxId": null, "DateTimeFormat": null }, { "Name": "Trace", "EndSeparator": null, "Type": 1, "ChildSyntaxId": null, "DateTimeFormat": null } ] } ], "LogFileType": 1 }'),

  ];
  public readonly defaultParserList: DefaultParserModel[] = this.parserList;
  constructor(private myParserEvents: ParserEventsAndUpdates) { 
    
  }

  OnParserClick(value:any) {
    const aDef = JSON.parse(value.logString);
    this.myParserEvents.SetCurrentParser(aDef, "preset");
  }
}

