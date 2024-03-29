# application-insights-sampling-demo
A simple demo about Sampling in Azure Application Insights


## Reduction Techniques
There are four data reduction techniques available in the Application Insights SDK. As a developer, you might utilize them using a built-in extensibility API. 

Metrics extraction and aggregation is a technique that lets you locally reduce data by aggregating metrics from telemetry data and sending only aggregated values, instead of the events themselves. Imagine you have 100 requests per minute. If the only thing you care about is the number of requests per minute, this technique would let you locally count the number of requests and send the value once a minute, instead of sending each request and calculating counts from the raw telemetry.

Sampling is a technique that selectively collects subsets of telemetry that lets you estimate the characteristics of the service. For most services you might collect every �n-th� request to get well-distributed statistical representation of service behavior. This technique, on the one hand, lets you reduce the volume of collection by �n� times, and on the other hand, preserves with certain accuracy statistical validity of the metrics derived from such telemetry. For better accuracy, a sophisticated algorithm and data model must be used.

Exemplification is the ability to collect samples of interest without invalidating sampling statistical accuracy. For example, you might want to always collect request failures regardless of sampling configuration. This way, while you reduce telemetry load with sampling, you can preserve useful troubleshooting data.

Filtering is the ability to reduce data by filtering out telemetry you don�t care about. For example, you might want to ignore all telemetry related to traffic generated by synthetic monitoring or search bots. This way, your metrics will reflect true user interaction with the service.