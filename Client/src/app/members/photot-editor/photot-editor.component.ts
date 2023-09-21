import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { Photo } from 'src/app/_models/Photo';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';
import { environment } from 'src/environments/environment.development';

@Component({
  selector: 'app-photot-editor',
  templateUrl: './photot-editor.component.html',
  styleUrls: ['./photot-editor.component.css']
})
export class PhototEditorComponent implements OnInit {

  @Input() member :Member |undefined;
  uploader:FileUploader |undefined
  hasBaseDropZoneOver=false;
  baseUrl = environment.apiUrl;
  user:User|undefined;
  constructor(private accountService:AccountService , private memberservice:MemberService)
  {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next : user=>
      {
        if(user) this.user = user;
      }
    })
  }
  ngOnInit(): void {
   this.initializeUploader();
  }
  fileOverBase(e:any)
  {
    this.hasBaseDropZoneOver = e;
  }
  setMainPhoto(photo :Photo)
  {
    this.memberservice.setMainPhoto(photo.id).subscribe({
      next:()=>
      {
        if(this.user && this.member)
        {
          this.user.photoUrl = photo.url;
          this.accountService.setCuutentUser(this.user);
          this.member.photoUrl = photo.url;
          this.member.photos.forEach(p=>
            {
              if(p.isMain == true) p.isMain =false;
              if(p.id == photo.id) p.isMain =true;
            })

        }

      }
    })

  }

  deletePhoto(photoId:number)
  {
    this.memberservice.deletePhoto(photoId).subscribe({
      next:_=> {
        if(this.member)
        {
          this.member.photos = this.member.photos.filter(x=> x.id !=photoId);
        }
      }
    })
  }

  initializeUploader()
  {
    this.uploader = new FileUploader({
      url : this.baseUrl + 'User/add-photo',
      authToken : 'Bearer ' + this.user?.token,
      isHTML5 : true,
      allowedFileType:['image'],
      removeAfterUpload : true ,
      autoUpload : false,
      maxFileSize : 10*1024*1024,

    });
    this.uploader.onBeforeUploadItem = (file) => {
      file.withCredentials = false;
    }


    // this.uploader.onAfterAddingAll = (file)=>
    // {
    //   file.withCredentials = false;
    // }

    this.uploader.onSuccessItem = (item ,response , status , headers) => {
      if(response)
      {
        const photo = JSON.parse(response);
        this.member?.photos.push(photo);
        if(photo.isMain &&this.user && this.member)
        {
          this.user.photoUrl = photo.url;
          this.member.photoUrl = photo.url;
          this.accountService.setCuutentUser(this.user);
        }
      }
    }
  }

}
