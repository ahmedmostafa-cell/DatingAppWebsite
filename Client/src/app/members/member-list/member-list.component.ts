import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userparams';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  // members$ : Observable<Member[]> | undefined;
  members : Member[]=[];
  pagination:Pagination|undefined;
  userparamse :UserParams | undefined;

  genderList = [{value:'male' , display:'Males'},{value:'female' , display:'Females'}];
  constructor(private memberService:MemberService ) {

      this.userparamse = this.memberService.getUserParams();
   }

  ngOnInit(): void {
  //  this.members$ = this.memberService.getMembers();
    this.loadMembers();
  }

  loadMembers()
  {
    if(this.userparamse)
    {
      this.memberService.setUserParams(this.userparamse);
      this.memberService.getMembers(this.userparamse).subscribe({
        next : response=>{
          if(response.result && response.pagination)
          {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
      })
    }

  }

  resetFilters()
  {

      this.userparamse = this.memberService.resetUserParams();
      this.loadMembers();

  }




  pageChnaged(event:any)
  {
    if(this.userparamse && this.userparamse?.pageNumber!=event.page)
    {
      this.userparamse.pageNumber = event.page;
      this.memberService.setUserParams(this.userparamse);
      this.loadMembers();
    }

  }



}
