import { Component } from '@angular/core';
import { Console } from 'console';

export class DefaultParserModel {
  public title: string;
  public preview: string;
  public logString: string;

  constructor(title: string, preview: string, logString: string)
  {
    this.title = title;
    this.preview = preview;
    this.logString= logString;
  }
}
