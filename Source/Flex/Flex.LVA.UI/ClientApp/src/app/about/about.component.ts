import { Component, OnInit } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent implements OnInit {

  pj = require('package.json');
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) { }
  version = this.pj.version;
  name = this.pj.name;

  data = [
    { name: 'App Name', value: this.name },
    { name: 'Version', value: this.version }
  ];

  ngOnInit(): void {
  }


}
