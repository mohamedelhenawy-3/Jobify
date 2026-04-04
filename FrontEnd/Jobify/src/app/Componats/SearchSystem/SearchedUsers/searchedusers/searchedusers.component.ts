import { CommonModule } from '@angular/common';
import { Component, Input, input } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-searchedusers',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './searchedusers.component.html',
  styleUrl: './searchedusers.component.css'
})
export class SearchedusersComponent {
@Input() users:any[]=[];
}
