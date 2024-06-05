import { Component, EventEmitter, Output } from '@angular/core';
import { DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-skip-header',
  templateUrl: './skip-header.component.html',
  styleUrls: ['./skip-header.component.css']
})
export class SkipHeaderComponent {
  headerSkipCount: number | undefined;;

  constructor(public ref: DynamicDialogRef) { }

  confirm(): void {
    this.ref.close(this.headerSkipCount);
  }

}
