import { apiDistrictList } from "@/services/settings/district";
import { LeftOutlined, PlusOutlined } from "@ant-design/icons";
import { PageContainer, ProTable } from "@ant-design/pro-components"
import { history } from "@umijs/max";
import { Button } from "antd";
import DistrictForm from "./components/form";

const Index: React.FC = () => {
    return (
        <PageContainer extra={<Button icon={<LeftOutlined />} onClick={() => history.back()}>Quay lại</Button>}>
            <ProTable
                headerTitle={<Button type="primary" icon={<PlusOutlined />}>Thêm mới</Button>}
                columns={[
                    {
                        title: "#",
                        valueType: 'indexBorder',
                        width: 30,
                        align: 'center'
                    },
                    {
                        title: 'Xã / Phường',
                        dataIndex: 'name'
                    }
                ]}
                search={{
                    layout: 'vertical'
                }}
                rowKey={`id`}
                request={apiDistrictList}
            />
            <DistrictForm />
        </PageContainer>
    )
}

export default Index;
