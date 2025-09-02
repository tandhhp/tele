import { PageContainer, ProTable } from "@ant-design/pro-components"

const Index: React.FC = () => {
    return (
        <PageContainer>
            <ProTable
                rowKey={`id`}
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
                        dataIndex: 'userName',
                    },
                    {
                        title: 'Level',
                        dataIndex: 'level',
                        search: false
                    },
                    {
                        title: 'Ngày',
                        dataIndex: 'createdDate',
                        search: false,
                        valueType: 'fromNow'
                    },
                    {
                        title: 'Nội dung',
                        dataIndex: 'message',
                        ellipsis: true
                    }
                ]}
            />
        </PageContainer>
    )
}

export default Index;