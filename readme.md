<a name="anchor-name-here" />
# Enterprise Integration Patterns with Windows Azure Service Bus #

This open-source library allows you to easily incorporate Enterprise Integration Patterns into your services using Processing Units and Channels, which can be assembled together to form sophisticated message processing pipelines. This library extends Windows Azure Service Bus vocabulary by explicitly separating processing logics from message queues and topics, instead of exclusively relying on existing rules and filters. The architecture allows a clearer separation of concerns so that most integration patterns can be implemented in a most straight-forward way.   

For instance, a Content-based Router is supposed to route messages to different recipients based on message contents. This behavior can be simulated using Windows Azure Service Bus subscription filters; however the simulated behavior has some key differences with an actual Content-based router:  
1)	The messages are broadcasted to all recipients and are only filtered out by recipient filters. While a Content-based router should route the message to one designated recipient.  
2)	The recipients need to know about (and contribute to) the routing logic, which is not their responsibility at all.   

For more complex patterns such as Dynamic Router, the routing logic needs access to more than one message channels and make decisions based on holistic view of participants. Scattering such logic to individual channels is not only complex and hard to maintain, but also impractical in some cases.   

The key idea of this library is to separate the processing logics into their justifiable entities - a Router is an independent processing unit, for instance. If you visualize Service Bus queues and topics as “pipes” where messages flow, the processing units are the “connectors” among these pipes, allowing much flexible, and in many cases more straight-forward, constructions of message pipelines.  

The main design goals of this library include:  

 - Separation of concern. Processing Unit is treated as a separate, first-class citizen.  
 - Scalable. Processing Units can be hosted in distributed servers, can be scaled separately, and can be migrated across server boundaries as needed.
 - Easy-to-use. You can use integration patterns in the most straight-forward way. You can create various kinds of processing units and message channels and assemble them into complete message pipelines.
 - Easy-to-manage.  You can configure the entire messaging pipeline using configuration file; you can also adjust the pipeline during runtime. 
 - Extensible. You can create as many custom processing units and message channels as you wish and they’ll work nicely with prebuilt ones.   

The following table summarizes the patterns supported by this library (as of 7/30/2012):

 **Pattern**|**Processing Unit**|**Input#**|**Output#**|**Control#**
 --------|--------|--------|--------|--------
 Content-Based Router|IntegrationPatterns.Routers.DynamicRouter|1|n|1
 Dynamic Router|IntegrationPatterns.Routers.DynamicRouter|1|n|1
