import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';
import { PresenceService } from 'src/app/_services/presence.service';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/_models/user';
import { take } from 'rxjs';

@Component({
  selector: 'app-member-detail',
  standalone:true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports:[CommonModule , TabsModule , GalleryModule , TimeagoModule , MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit , OnDestroy {
  @ViewChild('memberTabs' , {static:true}) memberTabs?: TabsetComponent;
  member : Member ={} as Member;
  images : GalleryItem[] =[];
  activeTab?: TabDirective;
  messages:Message[]=[];
  user?: User;
  constructor(private messageServie:MessageService, private accountService:AccountService , private route :ActivatedRoute , public presenceService : PresenceService)
  {
      this.accountService.currentUser$.pipe(take(1)).subscribe({
        next : user=>
        {
          if(user) this.user = user;
        }
      })
  }



  ngOnInit(): void {
    this.route.data.subscribe({
      next: data=> this.member = data['member']

    });
    this.route.queryParams.subscribe({
      next : params=>
      {
        params['tab'] && this.selectTab(params['tab']);
      }
    })
    this.getImages();
  }

  ngOnDestroy(): void {
    this.messageServie.stopHubConnection();
  }

  selectTab(heading:string)
  {

    if(this.memberTabs)
    {
      this.memberTabs.tabs.find(x=> x.heading===heading)!.active=true;
    }
  }
  OnTabActivated(data:TabDirective)
  {
    this.activeTab = data;
    if(this.activeTab.heading ==="Messages" && this.user)
    {
       this.messageServie.createHubConnection(this.user , this.member.userName);
    }
    else
    {
      this.messageServie.stopHubConnection();
    }
  }

  loadMessages()
  {
    if(this.member)
    {
      this.messageServie.getMessageThread(this.member.userName).subscribe({
        next: response=>
        {
          this.messages=response,
          console.log(this.messages)
        }


      })

    }

  }

  getImages()
  {
    if(!this.member) return;
    for(const photo of this.member.photos)
    {
      this.images.push(new ImageItem({src:photo.url , thumb:photo.url}));
    }

  }

}
function ngOnDestroy() {
  throw new Error('Function not implemented.');
}

