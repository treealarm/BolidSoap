using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

public class InspectorBehavior : IEndpointBehavior
{
  public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

  public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
  {
    clientRuntime.ClientMessageInspectors.Add(new MessageInspector());
  }

  public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

  public void Validate(ServiceEndpoint endpoint) { }
}
