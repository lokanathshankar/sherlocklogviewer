import { AfterViewInit, Component, OnInit} from '@angular/core';
import {RowComponent, TabulatorFull} from 'tabulator-tables';
import { ILogHeader, ISymanticLogs } from '../../common/loggraph';
import { Logger } from '../../services/logger.service';
import { Tracer } from '../../utils/tracer';


@Component({
  selector: 'app-flex-shell',
  templateUrl: './flex-shell.component.html',
  styleUrls: ['./flex-shell.component.css']
})
export class FlexShellComponent implements OnInit, AfterViewInit {
    ngAfterViewInit(): void {
    }
    ngOnInit(): void {
    }
}
