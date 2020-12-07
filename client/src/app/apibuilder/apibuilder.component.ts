import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';

import { DragulaService } from 'ng2-dragula';
import { Subscription, Observable, of } from "rxjs";
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';

import { OpenApiService } from '../services/openapi';

@Component({
  selector: 'app-apibuilder',
  templateUrl: './apibuilder.component.html',
  styleUrls: ['./apibuilder.component.css']
})

export class ApibuilderComponent implements OnInit {
  model: any;
  calcItems: any[] = [];
  arritems: any[] = [];
  dragItems: Array<any> = [];
  posDisplay: boolean = false;
  selectedItem: any;
  response: any = {};
  modalRef: BsModalRef | undefined;
  previewData: any;

  constructor(private dragulaService: DragulaService, private modalService: BsModalService, private openApiService: OpenApiService, private router: Router) {
    this.calcItems = [];
    this.model = this.router.getCurrentNavigation()?.extras.state;
    let tree = this.model.ParamTree as [];
    this.response.Operation = this.model.Operation;
    this.response.Verb = this.model.Verb;
    this.response.ParamTree = tree;
    this.response.Project = '';
    this.response.Design = {};
    this.response.Header = '';
    this.response.Server = '';

    for (let i = 0; i < tree.length; i++) {
      this.arritems.push({
        Name: (tree[i] as any).Name,
        Type: (tree[i] as any).Type,
        Node: (tree[i] as any).Node,
        Position: (tree[i] as any).Position,
        Values: (tree[i] as any).Values,
        Selected: 0,
        Id: i,
        Items: (tree[i] as any).Items,
        ObjectName: (tree[i] as any).ObjectName
      });
    }

    dragulaService.createGroup('OPENAPI', {
      copy: (el, source) => {
        return source.id === 'left';
      },
      copyItem: (item: any) => {
        return item;
      },
      accepts: (el, target, source, sibling) => {
        return target?.id !== 'left';
      },
      moves: (el, source, handle, sibling) => {
        if (el?.getAttribute("nodeType") == "object") {
          return false;
        }
        if (el?.getAttribute("nodeSelect") == "1") {
          return false;
        }
        return true;
      }
    });

    this.dragulaService.dropModel("OPENAPI").subscribe(args => {
      for (let i = 0; i < this.calcItems.length; i++) {
        for (let j = 0; j < this.calcItems[i].length; j++) {
          this.calcItems[i][j].Selected = 0;
          for (let k = 0; k < this.calcItems[i][j].Items.length; k++) {
            this.calcItems[i][j].Items[k].Selected = 0;
          }
        }
      }
      (args.item as any).Selected = 1;
      let item = {
        Name: args.item.Name,
        Level: args.item.Name,
        Type: args.item.Type,
        Node: args.item.Node,
        Id: args.item.Id,
        Selected: 1,
        Control: '0',
        Value: '',
        Required: false,
        Position: args.item.Position,
        Values: args.item.Values,
        Description: '',
        Error: '',
        Items: Array<any>(),
        ObjectName: args.item.Position+'_'+args.item.ObjectName,
      }
      for (let i = 0; i < args.item.Items.length; i++) {
        item.Items.push({
          Name: args.item.Items[i].Name,
          Level: args.item.Items[i].Name,
          Type: args.item.Items[i].Type,
          Node: args.item.Items[i].Node,
          Id: args.item.Items[i].Id,
          Selected: 0,
          Control: '0',
          Value: '',
          Required: false,
          Position: args.item.Items[i].Position,
          Values: args.item.Items[i].Values,
          Description: '',
          Error: '',
          ObjectName: args.item.Items[i].Position+'_'+args.item.Items[i].ObjectName,
          Items: []
        });
      }

      if (item.Type == 'array' && item.Items.length > 0) {
        item.Selected = 0;
        item.Items[0].Selected = 1;
      }

      if (args.target.className == "t" || args.target.className == "x") {
        const index: number = Number(args.target.getAttribute("row"));
        this.calcItems.splice(index, 0, [item]);
      }
      if (args.target.className == "b") {
        const index: number = Number(args.target.getAttribute("row"));
        this.calcItems.splice(index + 1, 0, [item]);
      }
      if (args.target.className == "l") {
        const index: number = Number(args.target.getAttribute("row"));
        this.calcItems[index].splice(0, 0, item);
      }
      if (args.target.className == "r") {
        const index: number = Number(args.target.getAttribute("row"));
        this.calcItems[index].splice(1, 0, item);
      }
      if (item.Type == 'array' && item.Items.length > 0) {
        this.selectedItem = item.Items[0];
      }
      else {
        this.selectedItem = item;
      }
      setTimeout(() => {
        if (this.selectedItem.Control == '0') {
          this.selectedItem.Control = (document.getElementById('controlDD') as HTMLSelectElement).options[0].getAttribute("value");
        }
      }, 100);
    });
    this.dragulaService.drag("OPENAPI").subscribe(args => {
      this.posDisplay = true;
    });
    this.dragulaService.drop("OPENAPI").subscribe(args => {
      this.posDisplay = false;
    });
    this.dragulaService.over("OPENAPI").subscribe((args) => {
      if (args.container.className == "t" || args.container.className == "b" || args.container.className == "l" || args.container.className == "r" || args.container.className == "x") {
        (args.container as HTMLElement).style.backgroundColor = "#00a4bd";
      }
    });
    this.dragulaService.out("OPENAPI").subscribe((args) => {
      if (args.container.className == "t" || args.container.className == "b" || args.container.className == "l" || args.container.className == "r" || args.container.className == "x") {
        (args.container as HTMLElement).style.backgroundColor = "white";
      }
    });
  }

  ngOnInit(): void {

  }

  itemSelect(item: any) {
    for (let i = 0; i < this.calcItems.length; i++) {
      for (let j = 0; j < this.calcItems[i].length; j++) {
        this.calcItems[i][j].Selected = 0;
        for (let k = 0; k < this.calcItems[i][j].Items.length; k++) {
          this.calcItems[i][j].Items[k].Selected = 0;
        }
      }
    }
    item.Selected = 1;
    this.selectedItem = item;
    setTimeout(() => {
      if (this.selectedItem.Control == '0') {
        this.selectedItem.Control = (document.getElementById('controlDD') as HTMLSelectElement).options[0].getAttribute("value");
      }
    }, 100);
    console.log(this.calcItems);
  }

  deleteItem(i: number, j: number, k: number, item: any) {
    if (k == -1) {
      if (this.calcItems[i].length == 1) {
        this.calcItems.splice(i, 1);
      }
      else {
        this.calcItems[i].splice(j, 1);
      }
      for (let index = 0; index < this.arritems.length; index++) {
        if (item.Id == this.arritems[index].Id) {
          this.arritems[index].Selected = 0;
        }
      }
    }
    else {
      item.Items.splice(k, 1);
      if (item.Items.length == 0) {
        if (this.calcItems[i].length == 1) {
          this.calcItems.splice(i, 1);
        }
        else {
          this.calcItems[i].splice(j, 1);
        }
        for (let index = 0; index < this.arritems.length; index++) {
          if (item.Id == this.arritems[index].Id) {
            this.arritems[index].Selected = 0;
          }
        }
      }
    }

    this.selectedItem = undefined;
  }

  calcMargin(item: any, type: number) {
    if (type == 1) {
      return (item.Node * 20) - 30;
    }
    return item.Node * 20;
  }

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }

  generateCode() {
    this.response.Design = this.calcItems;
    this.openApiService.GenerateCode({
      Header: this.response.Header, 
      Project: this.response.Project,
      Server: this.response.Server,
      Operation: this.model, 
      Design: this.calcItems
    }).subscribe((res) => {
      console.log(res);
    });
  }

  preview() {
    this.previewData = JSON.stringify({ Header: this.response.Header, Operation: this.model, Design: this.calcItems });
    setTimeout(() => {
      (document.getElementById("previewForm") as HTMLFormElement).submit();
    }, 100);
  }
}
