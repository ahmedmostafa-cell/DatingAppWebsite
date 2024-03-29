import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MemberService } from 'src/app/_services/member.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],

})
export class MemberCardComponent implements OnInit {


  @Input() member : Member | undefined

  constructor(private memberservice:MemberService , private toastr:ToastrService , public presenceService:PresenceService){}

  ngOnInit() {

  }

  addLike(memebr:Member)
  {
    this.memberservice.addLike(memebr.userName).subscribe({
      next:()=>this.toastr.success('You Have Liked ' + memebr.knownAs),
    })
  }

}
