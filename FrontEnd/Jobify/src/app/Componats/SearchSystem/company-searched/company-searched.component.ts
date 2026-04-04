import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-company-searched',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './company-searched.component.html',
  styleUrl: './company-searched.component.css'
})
export class CompanySearchedComponent {
  @Input() companies: any[] = [];

}
