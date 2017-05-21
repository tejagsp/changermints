<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="ChangerMintsCloudServer" generation="1" functional="0" release="0" Id="45ffd47f-8262-4cd0-9a94-49677215503a" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="ChangerMintsCloudServerGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="ChangerMintsWorkerRole:ChangerMintsEndpoint" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/LB:ChangerMintsWorkerRole:ChangerMintsEndpoint" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="ChangerMintsWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/MapChangerMintsWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="ChangerMintsWorkerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/MapChangerMintsWorkerRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:ChangerMintsWorkerRole:ChangerMintsEndpoint">
          <toPorts>
            <inPortMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRole/ChangerMintsEndpoint" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapChangerMintsWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapChangerMintsWorkerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="ChangerMintsWorkerRole" generation="1" functional="0" release="0" software="C:\Personal stuff\Codes\ChangerMintsCloudServer\ChangerMintsCloudServer\csx\Debug\roles\ChangerMintsWorkerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="ChangerMintsEndpoint" protocol="tcp" portRanges="4545" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;ChangerMintsWorkerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;ChangerMintsWorkerRole&quot;&gt;&lt;e name=&quot;ChangerMintsEndpoint&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="ChangerMintsWorkerRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="ChangerMintsWorkerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="ChangerMintsWorkerRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="3750e21f-d7c0-4ba5-9c3c-21183e0637c4" ref="Microsoft.RedDog.Contract\ServiceContract\ChangerMintsCloudServerContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="fde347ed-5c09-4557-a546-7ad7848a70e4" ref="Microsoft.RedDog.Contract\Interface\ChangerMintsWorkerRole:ChangerMintsEndpoint@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRole:ChangerMintsEndpoint" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>