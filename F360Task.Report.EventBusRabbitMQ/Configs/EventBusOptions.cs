﻿namespace F360Task.Report.EventBusRabbitMQ.Configs;

public class EventBusOptions
{
    public string SubscriptionClientName { get; set; }
    public int RetryCount { get; set; } = 10;
}
