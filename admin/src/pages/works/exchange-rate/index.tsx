import { PageContainer, ProCard } from "@ant-design/pro-components"
import { Empty } from "antd";
import { useState } from "react";
import ExchangeRateLabels from "./labels";

const ExchangeRate: React.FC = () => {

    const [tab, setTab] = useState<string>('content');

    return (
        <PageContainer>
            <ProCard
                tabs={{
                    activeKey: tab,
                    items: [
                        {
                            label: 'Content',
                            key: 'content',
                            children: <Empty />,
                        },
                        {
                            label: 'Labels',
                            key: 'labels',
                            children: <ExchangeRateLabels />,
                        },
                    ],
                    onChange: (key) => {
                        setTab(key);
                    },
                }}
            />
        </PageContainer>
    )
}

export default ExchangeRate;