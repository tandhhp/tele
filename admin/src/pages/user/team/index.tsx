import { apiMyTeam } from "@/services/user"
import { BRANCH_OPTIONS } from "@/utils/constants"
import { ManOutlined, UserOutlined, WomanOutlined } from "@ant-design/icons"
import { PageContainer, ProTable } from "@ant-design/pro-components"
import { Tag } from "antd"

const Index: React.FC = () => {
    return (
        <PageContainer>
            <ProTable
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30,
                    },
                    {
                        title: <UserOutlined />,
                        dataIndex: 'avatar',
                        valueType: 'avatar',
                        width: 30,
                        search: false
                    },
                    {
                        title: 'Tài khoản',
                        dataIndex: 'userName',
                        search: false
                    },
                    {
                        title: 'Họ và tên',
                        dataIndex: 'name',
                        valueType: 'text',
                        render: (dom, entity) => (
                            <div>
                                {entity.gender === true && (<ManOutlined className='text-blue-500' />)}{entity.gender === false && (<WomanOutlined className='text-red-500' />)} {dom}
                            </div>
                        )
                    },
                    {
                        title: 'Email',
                        dataIndex: 'email',
                        valueType: 'text',
                        search: false
                    },
                    {
                        title: 'Số điện thoại',
                        dataIndex: 'phoneNumber',
                        valueType: 'text',
                    },
                    {
                        title: 'Ngày sinh',
                        dataIndex: 'dateOfBirth',
                        valueType: 'date',
                        width: 100,
                        search: false
                    },
                    {
                        title: 'Chi nhánh',
                        dataIndex: 'branch',
                        valueType: 'select',
                        width: 100,
                        search: false,
                        fieldProps: {
                            options: BRANCH_OPTIONS
                        }
                    },
                ]}
                request={apiMyTeam}
                search={{
                    layout: 'vertical'
                }}
                rowKey="id"
            />
        </PageContainer>
    )
}

export default Index