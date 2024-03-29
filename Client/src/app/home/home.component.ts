import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registermode =false;
  users:any;
  constructor(private http : HttpClient ) { }

  ngOnInit(): void {

  }
  registermodeToggle()
  {
    this.registermode = ! this.registermode
  }


  cancelRegisterMode(event : boolean)
  {
    this.registermode = event;
  }

}
