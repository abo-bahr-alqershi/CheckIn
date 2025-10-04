using System;
using System.Text.Json.Serialization;
using MediatR;
using YemenBooking.Application.DTOs;
 
 namespace YemenBooking.Application.Commands.Chat
 {
     /// <summary>
     /// أمر لتعديل محتوى رسالة المحادثة
     /// Command to edit chat message content
     /// </summary>
     public class EditMessageCommand : IRequest<ResultDto<ChatMessageDto>>
     {
         /// <summary>
         /// معرّف الرسالة
         /// Message identifier
         /// </summary>
        [JsonIgnore]
        public Guid MessageId { get; set; }
 
         /// <summary>
         /// المحتوى الجديد للرسالة
         /// New content of the message
         /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
     }
 } 