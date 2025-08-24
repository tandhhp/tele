import { listRole } from "@/services/role"
import { UserAddOutlined } from "@ant-design/icons"
import { PageContainer, ProColumns, ProTable } from "@ant-design/pro-components"
import { history } from "@umijs/max"
import { Button } from "antd"

const RolePage: React.FC = () => {
    const columns: ProColumns<API.Role>[] = [
        {
            title: '#',
            valueType: 'indexBorder',
            width: 40,
            align: 'center'
        },
        {
            title: 'Quyền',
            dataIndex: 'displayName'
        },
        {
            title: 'Mô tả',
            dataIndex: 'description'
        },
        {
            title: 'Đang làm',
            dataIndex: 'total'
        },
        {
            title: 'Đã nghỉ',
            dataIndex: 'leave'
        },
        {
            title: 'Chi tiết',
            valueType: 'option',
            render: (dom, entity) => [
                <Button key="detail" type="primary" size='small' icon={<UserAddOutlined />} onClick={() => history.push(`/users/roles/${entity.name}`)}>Quản lý</Button>
            ],
            width: 100
        }
    ]
    
    return (
        <PageContainer>
            <ProTable request={listRole}
                columns={columns}
                search={false}
                scroll={{
                    x: true
                }}
            />
            
        </PageContainer>
    )
}

export default RolePage