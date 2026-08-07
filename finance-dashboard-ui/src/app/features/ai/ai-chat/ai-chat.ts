import {
  Component,
  ElementRef,
  ViewChild,
  inject
} from '@angular/core';

import { FormsModule } from '@angular/forms';

import { AiService } from '../../../core/services/ai';

interface ChatMessage {

  role: 'ai' | 'user';

  content: string;

}

@Component({
  selector: 'app-ai-chat',
  imports: [FormsModule],
  templateUrl: './ai-chat.html',
  styleUrl: './ai-chat.css',
})
export class AiChat{

  @ViewChild('messagesContainer')
  private readonly messagesContainer!:
    ElementRef<HTMLDivElement>;

  messages: ChatMessage[] = [];

  inputText = '';

  isTyping = false;

  private shouldScroll = false;

  private readonly aiService =
    inject(AiService);

  // ngAfterViewChecked(): void {

  //   if (this.shouldScroll) {

  //     this.scrollToBottom();

  //     this.shouldScroll = false;

  //   }

  // }

  send(): void {

    const text =
      this.inputText.trim();

    if (!text || this.isTyping) {

      return;

    }

    this.messages.push({

      role: 'user',

      content: text

    });

    this.inputText = '';

    this.isTyping = true;

    this.shouldScroll = true;

    this.aiService
      .askExpenses({

        question: text

      })
      .subscribe({

        next: (response) => {

          this.messages.push({

            role: 'ai',

            content:
              response.answer ??
              'No answer returned.'

          });

          this.isTyping = false;

          this.shouldScroll = true;

        },

        error: (error) => {

          console.error(error);

          this.messages.push({

            role: 'ai',

            content:
              'Sorry, I could not process your request.'

          });

          this.isTyping = false;

          this.shouldScroll = true;

        }

      });

  }

  sendChip(
    text: string
  ): void {

    this.inputText = text;

    this.send();

  }

  onEnter(
    event: Event
  ): void {

    const ke =
      event as KeyboardEvent;

    if (!ke.shiftKey) {

      ke.preventDefault();

      this.send();

    }

  }

  private scrollToBottom(): void {

    try {

      const el =
        this.messagesContainer
          ?.nativeElement;

      if (el) {

        el.scrollTop =
          el.scrollHeight;

      }

    }
    catch {

      // ignore

    }

  }

}
