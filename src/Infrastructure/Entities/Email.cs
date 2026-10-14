using Infrastructure.Enums;

namespace Infrastructure.Entities;

public class Email
{
    public int? ID {get; private set;}
    public int RecipientID {get; private set;}
    public string Content {get; private set;} = null!;
    public EmailStatus Status {get; set;}
    public DateTime CreatedAt {get; private set;}
    public DateTime SentAt {get; private set;}

    public Email(int recipientID, string content, EmailStatus status)
    {
        RecipientID = recipientID;
        Content = content;
        Status = status;
    }

    private Email()
    {
        
    }

    public void Send()
    {
        if (Status == EmailStatus.Sent)
        {
            return;
        }

        Status = EmailStatus.Sent;
        SentAt = DateTime.UtcNow;
    }
}
