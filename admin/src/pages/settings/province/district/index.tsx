import { apiDistrictList } from "@/services/settings/district";
import { LeftOutlined, PlusOutlined } from "@ant-design/icons";
import { ActionType, PageContainer, ProTable } from "@ant-design/pro-components"
import { history } from "@umijs/max";
import { Button } from "antd";
import DistrictForm from "./components/form";
import { useRef, useState } from "react";

const Index: React.FC = () => {

    const actionRef = useRef<ActionType>();
    const [open, setOpen] = useState<boolean>(false);

    return (
        <PageContainer extra={<Button icon={<LeftOutlined />} onClick={() => history.back()}>Quay lại</Button>}>
            <ProTable
                actionRef={actionRef}
                headerTitle={<Button type="primary" icon={<PlusOutlined />} onClick={() => setOpen(true)}>Thêm mới</Button>}
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
            <DistrictForm open={open} onOpenChange={setOpen} reload={() => actionRef.current?.reload()} />
        </PageContainer>
    )
}

export default Index;
