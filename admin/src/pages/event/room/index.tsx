import { PageContainer, ProTable } from "@ant-design/pro-components"

const Index: React.FC = () => {
    return (
        <PageContainer>
            <ProTable
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
                        title: 'Tên phòng',
                        dataIndex: 'name',
                    },
                    {
                        title: 'Sức chứa',
                        dataIndex: 'capacity',
                    },
                    {
                        title: 'Trạng thái',
                        dataIndex: 'status',
                    },
                ]}
            />
        </PageContainer>
    )
}

export default Index;