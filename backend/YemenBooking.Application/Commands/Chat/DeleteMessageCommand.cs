using System;
using System.Text.Json.Serialization;
using MediatR;
using YemenBooking.Application.DTOs;
 
 namespace YemenBooking.Application.Commands.Chat
 {
     /// <summary>
     /// أمر حذف رسالة المحادثة
     /// Command to delete a chat message by ID
     /// </summary>
     public class DeleteMessageCommand : IRequest<ResultDto>
     {
         /// <summary>
         /// معرّف الرسالة
         /// Message identifier
         /// </summary>
         [JsonPropertyName("message_id")]
         public Guid MessageId { get; set; }
     }
 } 