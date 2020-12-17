import { Component, OnInit, Input } from '@angular/core';

import { DragulaService } from 'ng2-dragula';

@Component({
  selector: 'app-response-design',
  templateUrl: './response-design.component.html',
  styleUrls: ['./response-design.component.css']
})
export class ResponseDesignComponent implements OnInit {
  @Input() model: any;
  public calcItems: any[] = [];
  arritems: any[] = [];
  dragItems: Array<any> = [];
  posDisplay: boolean = false;
  selectedItem: any;
  public gridView: boolean = false;
  gridViewDisable: boolean = false;

  constructor(private dragulaService: DragulaService) {
    dragulaService.createGroup('RESOPENAPI', {
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

    this.dragulaService.dropModel("RESOPENAPI").subscribe(args => {
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
        IsRequired: args.item.IsRequired,
        Selected: 1,
        Control: '0',
        Value: '',
        Required: args.item.IsRequired,
        Position: args.item.Position,
        Values: args.item.Values,
        Description: '',
        Error: '',
        Items: Array<any>(),
        ObjectName: args.item.ObjectName,
      }
      for (let i = 0; i < args.item.Items.length; i++) {
        item.Items.push({
          Parent: item,
          Name: args.item.Items[i].Name,
          Level: args.item.Items[i].Name,
          Type: args.item.Items[i].Type,
          Node: args.item.Items[i].Node,
          Id: args.item.Items[i].Id,
          IsRequired: args.item.Items[i].IsRequired,
          Selected: 0,
          Control: '0',
          Value: '',
          Required: args.item.Items[i].IsRequired,
          Position: args.item.Items[i].Position,
          Values: args.item.Items[i].Values,
          Description: '',
          Error: '',
          ObjectName: args.item.Items[i].ObjectName,
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
      console.log(this.calcItems);
      console.log(this.selectedItem);
    });
    this.dragulaService.drag("RESOPENAPI").subscribe(args => {
      this.posDisplay = true;
    });
    this.dragulaService.drop("RESOPENAPI").subscribe(args => {
      this.posDisplay = false;
    });
    this.dragulaService.over("RESOPENAPI").subscribe((args) => {
      if (args.container.className == "t" || args.container.className == "b" || args.container.className == "l" || args.container.className == "r" || args.container.className == "x") {
        (args.container as HTMLElement).style.backgroundColor = "#00a4bd";
      }
    });
    this.dragulaService.out("RESOPENAPI").subscribe((args) => {
      if (args.container.className == "t" || args.container.className == "b" || args.container.className == "l" || args.container.className == "r" || args.container.className == "x") {
        (args.container as HTMLElement).style.backgroundColor = "white";
      }
    });

    this.dragulaService.dropModel("RESOPENAPI").subscribe(args => {
      console.log(args);
    });
  }

  ngOnInit(): void {
    this.calcItems = [];
    let tree = this.model.ResponseTree as [];
    for (let i = 0; i < tree.length; i++) {
      this.arritems.push({
        Name: (tree[i] as any).Name,
        Type: (tree[i] as any).Type,
        Node: (tree[i] as any).Node,
        Position: (tree[i] as any).Position,
        Values: (tree[i] as any).Values,
        IsRequired: (tree[i] as any).IsRequired,
        Selected: 0,
        Id: i,
        Items: (tree[i] as any).Items,
        ObjectName: (tree[i] as any).ObjectName
      });
    }
    if (this.model.ResponseType == 'array') {
      this.gridView = true;
      this.gridViewDisable = true;
    }
  }

  calcMargin(item: any, type: number) {
    if (type == 1) {
      return (item.Node * 20) - 30;
    }
    return item.Node * 20;
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

  reset(e: any) {
    this.calcItems = [];
    this.selectedItem = undefined;
    for (let index = 0; index < this.arritems.length; index++) {
      this.arritems[index].Selected = 0;
    }
  }
}
