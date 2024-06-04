using OrionPro;
using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

public class MessageInspector : IClientMessageInspector
{
  public void AfterReceiveReply(ref Message reply, object correlationState)
  {
    // Сохраняем входящее сообщение
    SaveMessage(ref reply, "RESPONSE");
  }

  public object BeforeSendRequest(ref Message request, IClientChannel channel)
  {
    // Сохраняем исходящее сообщение
    SaveMessage(ref request, "REQUEST");
    return null;
  }

  private void SaveMessage(ref Message message, string messageType)
  {
    string filename = "log.xml";
    {
      MessageBuffer buffer = message.CreateBufferedCopy(Int32.MaxValue);
      Message copy = buffer.CreateMessage();
      message = buffer.CreateMessage();
      // Выводим копию сообщения в консоль
      using (var stringWriter = new StringWriter())
      using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
      {
        copy.WriteMessage(xmlWriter);
        xmlWriter.Flush();
        var s = stringWriter.GetStringBuilder().ToString();
        //Console.WriteLine(s);

        using (var fs = new FileStream(filename, FileMode.Append, FileAccess.Write))
        using (var writer = new StreamWriter(fs, Encoding.Unicode))
        {
          writer.WriteLine("");
          writer.WriteLine("=========================================");
          writer.WriteLine(messageType);
          writer.WriteLine("=========================================");
          writer.Write(s);
        }
      }
    }    
  }
}

