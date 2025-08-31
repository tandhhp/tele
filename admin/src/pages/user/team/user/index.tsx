import { apiTeamUsers } from "@/services/users/team";
import { LeftOutlined } from "@ant-design/icons";
import { ActionType, PageContainer, ProTable } from "@ant-design/pro-components"
import { useParams, history } from "@umijs/max";
import { Button } from "antd";
import TeamAddUserForm from "./components/add";
import { useRef } from "react";

const Index: React.FC = () => {

    const { id } = useParams<{ id: string }>();
    const actionRef = useRef<ActionType>(null);

    return (
        <PageContainer extra={<Button icon={<LeftOutlined />} onClick={() => history.back()}>Quay lại</Button>}>
            <ProTable
                actionRef={actionRef}
                rowKey="id"
                headerTitle={<TeamAddUserForm reload={() => actionRef.current?.reload()} />}
                request={apiTeamUsers}
                params={{ teamId: id }}
                search={{
                    layout: 'vertical'
                }}
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30,
                        align: 'center'
                    },
                    {
                        title: 'Tài khoản',
                        dataIndex: 'userName'
                    },
                    {
                        title: 'Họ tên',
                        dataIndex: 'name'
                    },
                    {
                        title: 'Ngày sinh',
                        dataIndex: 'dateOfBirth',
                        valueType: 'date',
                        search: false
                    },
                    {
                        title: 'SDT',
                        dataIndex: 'phoneNumber'
                    },
                    {
                        title: 'Email',
                        dataIndex: 'email'
                    }
                ]}
            />
        </PageContainer>
    )
}

export default Index;