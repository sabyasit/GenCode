import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AceEditorModule } from 'ng2-ace-editor';

import { HttpClientModule } from '@angular/common/http';

import { ModalModule } from 'ngx-bootstrap/modal';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { OpenApiService } from './services/openapi';
import { FormsModule } from '@angular/forms';

import { DragulaModule } from 'ng2-dragula';
import { ApibuilderComponent } from './apibuilder/apibuilder.component';
import { OpenapiComponent } from './openapi/openapi.component';

@NgModule({
  declarations: [
    AppComponent,
    ApibuilderComponent,
    OpenapiComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    AceEditorModule,
    HttpClientModule,
    ModalModule.forRoot(),
    DragulaModule.forRoot()
  ],
  providers: [OpenApiService],
  bootstrap: [AppComponent]
})
export class AppModule { }
