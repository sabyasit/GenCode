import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';

import * as _ from 'lodash';

import { Subscription, Observable, of } from "rxjs";
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';

import { OpenApiService } from '../services/openapi';

import { RequestDesignComponent } from '../request-design/request-design.component'
import { ResponseDesignComponent } from '../response-design/response-design.component';

@Component({
  selector: 'app-apibuilder',
  templateUrl: './apibuilder.component.html',
  styleUrls: ['./apibuilder.component.css']
})

export class ApibuilderComponent implements OnInit {
  @ViewChild(RequestDesignComponent) ReqDesign!: RequestDesignComponent;
  @ViewChild(ResponseDesignComponent) ResDesign!: ResponseDesignComponent;
  
  model: any;
  response: any = {
    Server: '',
    Header: '',
    Project: '',
    Operation: {},
    GridView: true,
    Design: [],
    ResDesign:[]
  };

  constructor(private router: Router, private modalService: BsModalService, private openApiService: OpenApiService) {
    this.model = this.router.getCurrentNavigation()?.extras.state;
    this.response.Server = this.model.Server[0];
  }

  ngOnInit(): void {
    document.getElementById("previewFrame")?.setAttribute("src", "http://localhost:3000/" + this.model.Preview);
  }

  buildResponse(){
    this.response.Operation = this.model;
    this.response.Design = this.ReqDesign.calcItems;
    this.response.ResDesign = _.cloneDeep(this.ResDesign.calcItems);
    for (let i = 0; i < this.response.ResDesign.length; i++) {
      for (let j = 0; j < this.response.ResDesign[i].length; j++) {
        for (let k = 0; k < this.response.ResDesign[i][j].Items.length; k++) {
          this.response.ResDesign[i][j].Items[k].Parent = null;
        }
      }
    }
    this.response.GridView = this.ResDesign.gridView;
  }

  generateCode() {
    this.buildResponse();
    this.openApiService.GenerateCode(this.response).subscribe((res) => {
      window.location.href = 'api/export/' + res;
    });
  }

  preview() {
    let require: string = '';
    for (let i = 0; i < this.ReqDesign.arritems.length; i++) {
      if (this.ReqDesign.arritems[i].IsRequired) {
        let present: boolean = false;
        for (let j = 0; j < this.ReqDesign.calcItems.length; j++) {
          for (let k = 0; k < this.ReqDesign.calcItems[j].length; k++) {
            if ((this.ReqDesign.arritems[i].Position + "_" + this.ReqDesign.arritems[i].ObjectName) == this.ReqDesign.calcItems[j][k].ObjectName) {
              present = true;
            }
          }
        }
        if (!present) {
          require = require + this.ReqDesign.arritems[i].Name + ", "
        }
      }
    }
    if (require.length > 0) {
      alert(require);
      return;
    }
    this.buildResponse();
    console.log(this.response);
    //return;
    this.openApiService.PreviewCode(this.model.Preview, this.response).subscribe((res) => {

    });
  }
}
