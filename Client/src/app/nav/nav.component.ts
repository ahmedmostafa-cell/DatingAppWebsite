import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model :any = {}

  constructor(public accountservice:AccountService) { }

  ngOnInit(): void {

  }


  logIn() {
    this.accountservice.login(this.model).subscribe({
      next: response => {
        console.log(response);

      },
      error: error => {
        console.log(error);
      }

    })
  }

  loggedOut()
  {
    this.accountservice.logout();


  }

}
