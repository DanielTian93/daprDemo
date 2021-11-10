namespace daprA.Controllers.Secrets
{
    public class SecretsController
    {
    }
    /*
     https://docs.dapr.io/zh-hans/reference/components-reference/supported-secret-stores/
                Dictionary<string, string> secrets = await _daprClient.GetSecretAsync("secretsname", "RabbitMQConnectStr");
                return Ok(secrets["RabbitMQConnectStr"]);

                从IConfiguration读取
                新增Nuget包 Dapr.Extensions.Configuration
                .ConfigureAppConfiguration(config =>
                {
                    var daprClient = new DaprClientBuilder().Build();
                    var secretDescriptors = new List<DaprSecretDescriptor> { new DaprSecretDescriptor("RabbitMQConnectStr") };
                    config.AddDaprSecretStore("secrets", secretDescriptors, daprClient);
                })
                .ConfigureAppConfiguration((ht, co) =>
                {
                    ht.Configuration = co.Build();
                })
                (_configuration["RabbitMQConnectStr"]

                组件引用Secrets,修改rabbitmq-input-binding.yaml
                    apiVersion: dapr.io/v1alpha1
                    kind: Component
                    metadata:
                      name: api/RabbitBinding
                    spec:
                      type: bindings.rabbitmq
                      version: v1
                      metadata:
                      - name: queueName
                        value: queue-for-input-binding
                      - name: host
                        secretKeyRef:
                          name: RabbitMQConnectStr
                          key: RabbitMQConnectStr
                    auth:
                      secretStore: secrets01


                在dapr的config.yaml中修改
                    secrets:
                        scopes:
                          - storeName: secrets01
                            defaultAccess: deny --deny 拒绝访问
                    或者
                     - storeName: secrets01
                            defaultAccess: deny 
                            allowedSecrets: ["RabbitMQConnectStr"] 允许其中的XX可以访问


     */
}
