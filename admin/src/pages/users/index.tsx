import { apiUserList } from "@/services/user";
import { apiTeamOptions } from "@/services/users/team";
import { UserAddOutlined } from "@ant-design/icons";
import { ActionType, PageContainer, ProTable } from "@ant-design/pro-components"
import { Button } from "antd";
import { useRef } from "react";

const Index: React.FC = () => {

    const actionRef = useRef<ActionType>();
    
    return (
        <PageContainer extra={<Button type="primary" icon={<UserAddOutlined />}>Thêm người dùng</Button>}>
             <ProTable
                search={{
                    layout: 'vertical'
                }}
                request={apiUserList}
                 columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30,
                        align: 'center'
                    },
                    {
                        title: 'Họ và tên',
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
                        dataIndex: 'phoneNumber',
                        search: false
                    },
                    {
                        title: 'Email',
                        dataIndex: 'email',
                        search: false
                    },
                    {
                        title: 'Nhóm',
                        dataIndex: 'teamId',
                        valueType: 'select',
                        request: apiTeamOptions,
                        search: false
                    }
                 ]}
                actionRef={actionRef} />
        </PageContainer>
    )
}

export default Index;