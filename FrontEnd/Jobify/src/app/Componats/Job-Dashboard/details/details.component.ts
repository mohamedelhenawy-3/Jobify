import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './details.component.html',
  styleUrl: './details.component.css'
})
export class DetailsComponent {
  @Input() job: any | null = null;

  @Output() applyClicked = new EventEmitter<number>();
  @Output() saveToggle = new EventEmitter<number>();


  formatList(text: string): string[] {
    if (!text) return [];
    return text.split('\n').filter(item => item.trim().length > 0);
  }
}
