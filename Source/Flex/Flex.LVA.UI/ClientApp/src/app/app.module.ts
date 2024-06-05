import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { TabulatorGridComponent } from './tabulator-grid/tabulator-grid.component';
import { FlexShellComponent } from './flex-shell/flex-shell.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {MatButtonModule} from '@angular/material/button';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatMenuModule} from '@angular/material/menu';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatDividerModule} from '@angular/material/divider';
import {MatExpansionModule} from '@angular/material/expansion';
import {MatListModule} from '@angular/material/list';
import {MatIconModule} from '@angular/material/icon';
import { AngularSplitModule } from 'angular-split';
import { AppShellComponent } from './app-shell/app-shell.component';
import { TopMenuComponent } from './top-menu/top-menu.component';
import { LeftMenuComponent } from './left-menu/left-menu.component';
import { TableShellComponent } from './table-shell/table-shell.component';
import { InfoAreaComponent } from './info-area/info-area.component';
import { MatTabsModule } from '@angular/material/tabs';
import { MenubarModule } from 'primeng/menubar';
import { TabViewModule } from 'primeng/tabview';
import { MegaMenuModule } from 'primeng/megamenu';
import { TabMenuModule } from 'primeng/tabmenu';
import { TableViewComponent } from './table-view/table-view.component';
import { FindPopupComponent } from './find-popup/find-popup.component';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { FindViewComponent } from './find-view/find-view.component';
import { Splitter, SplitterModule } from 'primeng/splitter';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { RadioButtonModule } from 'primeng/radiobutton';
import { GlobalProgressBarComponent } from './global-progress-bar/global-progress-bar.component';
import { ProgressBarModule } from 'primeng/progressbar';
import { ColumnOptionsComponent } from './column-options/column-options.component';
import { ColumnOptionsPopupComponent } from './column-options-popup/column-options-popup.component';
import { TableModule } from 'primeng/table';
import { CheckboxModule } from 'primeng/checkbox';
import { FilterPopupComponent } from './filter-popup/filter-popup.component';
import { FilterViewComponent } from './filter-view/filter-view.component';
import { ToastModule } from 'primeng/toast';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ParserSetupViewComponent } from './parser-setup-view/parser-setup-view.component';
import { InputNumberModule } from 'primeng/inputnumber';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { DialogService, DynamicDialogModule, DynamicDialogRef } from 'primeng/dynamicdialog';
import { SyntaxSetupViewComponent } from './syntax-setup-view/syntax-setup-view.component';
import { ConfirmDialog, ConfirmDialogModule } from 'primeng/confirmdialog';
import { InvestigationViewComponent } from './investigation-view/investigation-view.component';
import { TimelineModule } from 'primeng/timeline';
import { CardModule } from 'primeng/card';
import { MenuModule } from 'primeng/menu';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { EditNoteComponent } from './edit-note/edit-note.component';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { FieldsetModule } from 'primeng/fieldset';
import { ScrollPanelModule } from 'primeng/scrollpanel';
import { TooltipModule } from 'primeng/tooltip';
import { SessionLoginComponent } from './session-login/session-login.component';
import { UserSettingsComponent } from './user-settings/user-settings.component';
import { ParserLoaderComponent } from './parser-loader/parser-loader.component';
import { EditorModule } from 'primeng/editor';
import { InvestigationPrintComponent } from './investigation-print/investigation-print.component';
import { DefaultParserComponent } from './default-parser/default-parser.component';
import { VirtualScrollerModule } from 'primeng/virtualscroller';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { AboutComponent } from './about/about.component';
import { SkipHeaderComponent } from './skip-header/skip-header.component';
@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    TabulatorGridComponent,
    FlexShellComponent,
    AppShellComponent,
    TopMenuComponent,
    LeftMenuComponent,
    TableShellComponent,
    InfoAreaComponent,
    TableViewComponent,
    FindPopupComponent,
    FindViewComponent,
    GlobalProgressBarComponent,
    ColumnOptionsComponent,
    ColumnOptionsPopupComponent,
    FilterPopupComponent,
    FilterViewComponent,
    ParserSetupViewComponent,
    SyntaxSetupViewComponent,
    InvestigationViewComponent,
    EditNoteComponent,
    SessionLoginComponent,
    UserSettingsComponent,
    ParserLoaderComponent,
    InvestigationPrintComponent,
    DefaultParserComponent,
    AboutComponent,
    SkipHeaderComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
    ]),
    
    MatFormFieldModule,
    MatButtonModule,
    MatInputModule,
    BrowserAnimationsModule,
    MatToolbarModule,
    MatIconModule,
    MatMenuModule,
    MatSidenavModule,
    MatDividerModule,
    MatExpansionModule,
    MatListModule,
    AngularSplitModule,
    MatTabsModule,
    MenubarModule,
    TabViewModule,
    TabMenuModule,
    MegaMenuModule,
    DialogModule,
    ButtonModule,
    SplitterModule,
    InputTextModule,
    DropdownModule,
    RadioButtonModule,
    ProgressBarModule,
    TableModule,
    CheckboxModule,
    MessagesModule,
    MessageModule,
    ToastModule,
    InputNumberModule,
    OverlayPanelModule,
    DynamicDialogModule,
    ConfirmDialogModule,
    TimelineModule,
    CardModule,
    MenuModule,
    ConfirmPopupModule,
    InputTextareaModule,
    FieldsetModule,
    ScrollPanelModule,
    TooltipModule,
    EditorModule,
    VirtualScrollerModule,
    ToggleButtonModule
  ],
  providers: [MessageService, DialogService, DynamicDialogRef, ConfirmationService],
  bootstrap: [AppComponent]
})
export class AppModule { }
