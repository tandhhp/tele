import { apiBranchList } from "@/services/settings/branch";
import { PageContainer, ProTable } from "@ant-design/pro-components"

const Index: React.FC = () => {
    return (
        <PageContainer>
            <ProTable
                rowKey="id"
                search={{
                    layout: 'vertical'
                }}
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30,
                    },
                    {
                        title: 'Chi nhánh',
                        dataIndex: 'name'
                    },
                    {
                        title: 'Xã/Phường',
                        dataIndex: 'districtName',
                        search: false
                    },
                    {
                        title: 'Phòng ban',
                        dataIndex: 'departmentCount',
                        search: false
                    }
                ]}
                request={apiBranchList}
            />
        </PageContainer>
    )
}

export default Index;