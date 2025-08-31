import { apiDistrictList } from "@/services/settings/district";
import { DeleteOutlined, LeftOutlined, PlusOutlined, SettingOutlined } from "@ant-design/icons";
import { ActionType, PageContainer, ProTable } from "@ant-design/pro-components"
import { history } from "@umijs/max";
import { Button, Popconfirm } from "antd";
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
                    },
                    {
                        title: <SettingOutlined />,
                        valueType: 'option',
                        render: (dom, entity) => [
                            <Popconfirm key={`delete`} title="Xác nhận xóa?">
                                <Button icon={<DeleteOutlined />} size="small" type="primary" danger />
                            </Popconfirm>
                        ],
                        width: 60
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
