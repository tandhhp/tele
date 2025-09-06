import { apiCallHistories } from "@/services/call";
import { apiContactGet } from "@/services/contact";
import { LeftOutlined } from "@ant-design/icons";
import { PageContainer, ProTable } from "@ant-design/pro-components"
import { history, useParams, useRequest } from "@umijs/max";
import { Button } from "antd";

const Index: React.FC = () => {

    const { id } = useParams<{ id: string }>();
    const { data } = useRequest(() => apiContactGet(id));

    return (
        <PageContainer title={data?.name} subTitle={data?.phoneNumber} extra={<Button icon={<LeftOutlined />} onClick={() => history.back()}>Quay lại</Button>}>
            <ProTable
                request={apiCallHistories}
                search={{
                    layout: 'vertical'
                }}
                rowKey="id"
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30,
                        align: 'center'
                    },
                    {
                        title: 'Ngày gọi',
                        dataIndex: 'createdDate',
                        valueType: 'dateTime',
                        search: false
                    },
                    {
                        title: 'Trạng thái',
                        dataIndex: 'callStatus',
                        search: false
                    },
                    {
                        title: 'Ngày theo dõi',
                        dataIndex: 'followUpDate',
                        valueType: 'dateTime',
                        search: false
                    },
                    {
                        title: 'Trạng thái bổ sung',
                        dataIndex: 'extraStatus',
                        search: false
                    },
                    {
                        title: 'Công việc',
                        dataIndex: 'job',
                        search: false
                    },
                    {
                        title: 'Thói quen du lịch',
                        dataIndex: 'travelHabit',
                        search: false
                    },
                    {
                        title: 'Tuổi',
                        dataIndex: 'age',
                        search: false
                    },
                    {
                        title: 'Ghi chú',
                        dataIndex: 'note',
                        search: false
                    }
                ]}
            />
        </PageContainer>
    )
}

export default Index;